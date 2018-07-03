FROM analyticsearth/sse2rservecore

COPY installpackages.r /root/src/scripts/installpackages.r
WORKDIR /root/src/scripts

RUN Rscript -e installpackages.r
