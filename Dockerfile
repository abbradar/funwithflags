FROM ubuntu:16.10

RUN apt-get update \
  && apt-get -y dist-upgrade \
  && apt-get -y install dirmngr apt-transport-https \
  && echo "deb [arch=amd64] https://apt-mo.trafficmanager.net/repos/dotnet-release/ yakkety main" > /etc/apt/sources.list.d/dotnetdev.list \
  && apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 417A0893 \
  && apt-get update \
  && apt-get -y install dotnet-dev-1.0.0-preview2-1-003177

COPY . /root
WORKDIR /root
RUN dotnet restore && dotnet build

EXPOSE 5000
CMD [ "dotnet", "run" ]
