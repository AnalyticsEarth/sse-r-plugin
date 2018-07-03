FROM analyticsearth/sse2rservecore

COPY installpackages.r /usr/local/src/scripts
WORKDIR /usr/local/src/scripts

RUN Rscript -e installpackages.r
