copy ComicApi\Dockerfile Dockerfile /Y
docker build . -t comic
docker login 
docker tag comic aluc243/comic
docker push aluc243/comic