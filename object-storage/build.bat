@echo off
cls

rem ***********************************************************
rem This batch file will build the Minio blob / object storage
rem image and start the container. This uses server nano
rem to run on docker for windows. 
rem        These values are for debug / dev only
rem ***********************************************************

copy /y Dockerfile-windows-nano Dockerfile

docker rm -f storage
docker rmi -f stor

docker build --tag stor . 
docker run -d -p 8080:9000 --name storage -e MINIO_ACCESS_KEY=abc -e MINIO_SECRET_KEY=abc123def --mount type=bind,source=D:\minio,target=C:\temp\cache_root stor