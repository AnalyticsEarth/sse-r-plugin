FROM analyticsearth/sse2rservecore

COPY installpackages.r /root/installpackages.r

RUN apt-get update \
  && apt-get install -y libnlopt-dev libcurl4-openssl-dev libxml2-dev \
  && apt-get clean \
  && Rscript "/root/installpackages.r" \
  && rm -rf /tmp/*
