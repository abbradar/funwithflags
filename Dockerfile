FROM ubuntu:16.04

# Update, install dependencies
RUN apt-get update \
  && apt-get -y dist-upgrade \
  && apt-get -y install dirmngr apt-transport-https gnupg-curl locales gosu \
  && localedef -i en_US -c -f UTF-8 -A /usr/share/locale/locale.alias en_US.UTF-8
ENV LANG en_US.utf8

# Install supervisord
RUN apt-get -y install supervisor

# Install PostgreSQL
ENV PG_MAJOR 9.6
RUN echo "deb http://apt.postgresql.org/pub/repos/apt/ xenial-pgdg main" > /etc/apt/sources.list.d/pgdg.list \
  && apt-key adv --fetch-keys https://www.postgresql.org/media/keys/ACCC4CF8.asc \
  && apt-get update \
  && apt-get -y install postgresql-$PG_MAJOR
ENV PATH /usr/lib/postgresql/$PG_MAJOR/bin:$PATH
ENV PGDATA /srv/postgresql
RUN mkdir -p "$PGDATA" && chown -R postgres:postgres "$PGDATA"
RUN gosu postgres initdb
# FIXME: Npgsql.EntityFrameworkcore.PostgreSQL has not yet been updated to support UNIX sockets.
# RUN sed -i 's/trust/ident/g' "$PGDATA/pg_hba.conf"
ENV DATABASE Host=127.0.0.1;Username=app

# Install .NET Core
RUN echo "deb [arch=amd64] https://apt-mo.trafficmanager.net/repos/dotnet-release/ xenial main" > /etc/apt/sources.list.d/dotnetdev.list \
  && apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 417A0893 \
  && apt-get update \
  && apt-get -y install dotnet-dev-1.0.1

# Create user
RUN useradd -m -g users app
WORKDIR /home/app
RUN set -e; \
  gosu postgres pg_ctl start; \
  while ! gosu postgres psql -c ""; do sleep 1; done; \
  gosu postgres createuser app; \
  gosu postgres createdb -O app app; \
  gosu postgres pg_ctl stop

# Update dotnet cache.
WORKDIR /tmp
RUN gosu app dotnet new -all

# Set up application
WORKDIR /home/app
COPY . /home/app
RUN chown -R app:users /home/app
# RUN git clean -ffdx
RUN gosu app dotnet restore; \
  gosu app dotnet build

# Initialize database.
RUN set -e; \
  gosu postgres pg_ctl start; \
  while ! gosu postgres psql -c ""; do sleep 1; done; \
  gosu app dotnet ef migrations add InitialMigration; \
  gosu app dotnet ef database update; \
  gosu postgres pg_ctl stop

EXPOSE 5000
VOLUME /srv/postgresql
CMD supervisord -c /home/app/supervisord.conf
