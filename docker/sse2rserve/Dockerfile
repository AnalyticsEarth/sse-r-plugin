FROM mono

ENV RHOST="rserve"

MAINTAINER "AnalyticsEarth" bbb@qlik.com

# Details
LABEL version="1.1.0"
LABEL description="Image running SSEtoRserve in mono"

# Initial setup
RUN apt-get update && apt-get upgrade -y
RUN apt-get install git unzip supervisor -y

# Get SSEtoRserve sources
RUN git clone https://github.com/AnalyticsEarth/sse-r-plugin.git

# Setup build environment
WORKDIR /sse-r-plugin/csharp
RUN nuget restore \
 && temp_dir=packages/Grpc.Tools.1.7.3/tmp \
 && curl_url=https://www.nuget.org/api/v2/package/Grpc.Tools/ \
 && mkdir -p $temp_dir \
 && cd $temp_dir \
 && curl -sL $curl_url > tmp.zip; unzip tmp.zip \
 && cd .. \
 && cp -r tmp/tools . \
 && rm -rf tmp \
 && cd ../.. \
 && chmod 755 packages/Grpc.Tools.1.7.3/tools/linux_x64/protoc \
 && chmod 755 packages/Grpc.Tools.1.7.3/tools/linux_x64/grpc_csharp_plugin \
 && chmod 755 ServerSideExtension/generate_SSE_Stubs_from_proto.sh

# Build application
RUN msbuild ServerSideExtension.sln /p:Configuration=Linux

# Cleanup
RUN rm -rf ServerSideExtension packages

# Port SSEtoRserve is going to listen on
EXPOSE ${PORT:-50051}

# Create configuration file for SSEtoRserve
RUN cp -rf /sse-r-plugin/docker/sse2rserve/sse2rserve.config /sse-r-plugin/csharp/SSEtoRserve/bin/Linux/SSEtoRserve.exe.tmpl

# Create start script
RUN { \
    echo "#!/bin/sh";\
    echo "export IPADDR=\`getent hosts \$RHOST | awk '{ print \$1 ; exit }'\`";\
    echo "export IPADDR=\${IPADDR:-127.0.0.1}";\
echo "sed 's/\$rServeHost/'\$IPADDR'/' /sse-r-plugin/csharp/SSEtoRserve/bin/Linux/SSEtoRserve.exe.tmpl > /sse-r-plugin/csharp/SSEtoRserve/bin/Linux/SSEtoRserve.exe.config";\
    echo "/usr/bin/mono /sse-r-plugin/csharp/SSEtoRserve/bin/Linux/SSEtoRserve.exe";\
} | tee /sse-r-plugin/csharp/start.sh
RUN chmod 755 /sse-r-plugin/csharp/start.sh

# Start command
ENTRYPOINT ["/bin/sh", "-c", "/sse-r-plugin/csharp/start.sh"]
