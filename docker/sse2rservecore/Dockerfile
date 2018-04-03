FROM microsoft/dotnet:2.1-runtime AS dotnet_r_base

RUN apt-get update \
  && apt-get upgrade -y \
  && apt-get install r-base \
  && apt-get clean

RUN R CMD install.packages("Rserve")

FROM microsoft/dotnet:2.1-sdk as builder
MAINTAINER "AnalyticsEarth" bbb@qlik.com

# Details
LABEL version="1.1.0"
LABEL description="SSEtoRserve on dotnetcore with self spawning R instance"

RUN mkdir -p /root/src/app/sse2rserve
WORKDIR /root/src/app/sse2rserve

COPY ../csharp/SSEtoRserveCore/SSEtoRserveCore.csproj SSEtoRserveCore/SSEtoRserveCore.csproj
RUN dotnet restore SSEtoRserveCore/SSEtoRserveCore.csproj

COPY ../csharp/SSEtoRserveCore/ .
RUN dotnet publish -c release -o published

FROM dotnet_r_base

WORKDIR /root/
COPY --from=builder /root/src/app/sse2rserve/published .

EXPOSE 50051/tcp
EXPOSE 19345/tcp
CMD ["dotnet","./SSEtoRserveCore.dll"]
