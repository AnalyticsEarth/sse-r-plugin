#inPack <- c("jsonlite","ChainLadder","forecast","rpart","d3r","partykit","tseries","colorspace")
inPack <- c("curl")

install.packages(inPack, dependencies=TRUE, repos='https://cloud.r-project.org/')


list.files(tempdir())
