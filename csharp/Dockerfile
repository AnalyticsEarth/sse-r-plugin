FROM microsoft/dotnet:2.1.5-runtime AS dotnet_r_base

RUN apt-get update \
  && apt-get upgrade -y \
  && apt-get install -y libssl-dev r-base \
  && apt-get clean

ADD https://www.rforge.net/Rserve/snapshot/Rserve_1.8-6.tar.gz .
RUN R CMD INSTALL Rserve_1.8-6.tar.gz

FROM microsoft/dotnet:2.1.403-sdk as builder

LABEL author="AnalyticsEarth"

# Details
LABEL version="1.1.0"
LABEL description="SSEtoRserve on dotnetcore with self spawning R instance"

RUN mkdir -p /root/src/app/sse2rserve
WORKDIR /root/src/app/sse2rserve

COPY SSEtoRServeCore/SSEtoRServeCore.csproj SSEtoRServeCore/SSEtoRServeCore.csproj
RUN dotnet restore SSEtoRServeCore/SSEtoRServeCore.csproj

COPY SSEtoRServeCore/ .
RUN dotnet publish -c release -o published

FROM dotnet_r_base

RUN groupadd -r sse2rserve \
 && useradd -g sse2rserve --create-home --shell /bin/bash sse2rserve

WORKDIR /root/
COPY --from=builder /root/src/app/sse2rserve/published .
RUN chown sse2rserve:sse2rserve . && \
  chmod +x -R .

EXPOSE 50051/tcp
EXPOSE 19345/tcp
USER sse2rserve
#CMD ./runservices.sh
