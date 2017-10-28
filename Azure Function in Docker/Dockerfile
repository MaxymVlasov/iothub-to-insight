FROM node:8

# Install .NET Core SDK 2.0 
RUN apt-get update && \
    apt-get -y install curl libunwind8 gettext apt-transport-https && \
    curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg && \
    mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg && \
    sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-debian-jessie-prod jessie main" > /etc/apt/sources.list.d/dotnetdev.list' && \
    apt-get update && \
    apt-get -y install dotnet-sdk-2.0.0

# Install azure-functions-cli
RUN npm i azure-functions-core-tools@core && \
    alias func='node /node_modules/azure-functions-core-tools/lib/main.js' >> /root/.bashrc 

EXPOSE 7071
WORKDIR /app

COPY . /app
