FROM analyticsearth/sse2rservecore

COPY installpackages.r /root/src/scripts/installpackages.r
WORKDIR /root/src/scripts

RUN apt-get install -y libnlopt-dev

RUN Rscript "/root/src/scripts/installpackages.r"
