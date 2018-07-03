FROM analyticsearth/sse2rservecore

COPY installpackages.r /root/src/scripts
WORKDIR /root/src/scripts

RUN Rscript -e installpackages.r
