FROM analyticsearth/sse2rservecore

COPY installpackages.r /root/src/scripts/installpackages.r

RUN apt-get update \
  && apt-get install -y libnlopt-dev libcurl4-openssl-dev

RUN Rscript "/root/src/scripts/installpackages.r"
