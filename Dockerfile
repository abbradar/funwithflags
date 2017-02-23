FROM ubuntu:16.04

# Update, install dependencies
RUN apt-get update \
  && apt-get -y dist-upgrade \
  && apt-get -y install dirmngr apt-transport-https gnupg-curl locales supervisor \
  && localedef -i en_US -c -f UTF-8 -A /usr/share/locale/locale.alias en_US.UTF-8

ENV LANG en_US.utf8

# Install PostgreSQL
RUN echo "deb http://apt.postgresql.org/pub/repos/apt/ xenial-pgdg main" > /etc/apt/sources.list.d/pgdg.list \
  && apt-key adv --fetch-keys https://www.postgresql.org/media/keys/ACCC4CF8.asc \
  && apt-get update \
  && apt-get -y install postgresql-9.6

# Install .NET Core
RUN echo "deb [arch=amd64] https://apt-mo.trafficmanager.net/repos/dotnet-release/ xenial main" > /etc/apt/sources.list.d/dotnetdev.list \
  && apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 417A0893 \
  && apt-get update \
  && apt-get -y install dotnet-dev-1.0.0-preview2-1-003177

COPY . /root
WORKDIR /root
RUN dotnet restore && dotnet build

EXPOSE 5000
RUN echo '{ "server.urls": "http://0.0.0.0:5000" }' > hosting.json
CMD [ "dotnet", "run" ]
