# Docker Test connection
 Docker Test connection is a frame of netcore project. It also 
 support to test connection to other services that the inner 
 or outter network. It support test MsSql, Mysql, Mongo, redis .etc. 

## Development enviroment
Visual studio Community 2022 (Version 17.12.3)

.net 8.x

.npm version 10.8.2

IIS express && docker
## Support configrations for net core

Support config dockerfile

Support config Nlog

Support config swagger.

Support config multi-services with the same interface
## Step by step for running
### IIS Express running
1. Download source codes

2. open solution with Visual studio

3. Visual studio => build solution

4. Visual studio => run

5. open the swagger url: [https://localhost:8081/](https://localhost:8081/)

### Docker running
docker build . --file Dockerfile --tag dockertestconnect

docker save dockertestconnect -o dockertestconnect.tar

Upload file "dockertestconnect.tar" to server

docker load -i dockertestconnect.tar

docker run -d --name dockertestconnect \
           -v /home/appdata/dockertestconnect/logs:/app/logs \
           -v /home/appdata/dockertestconnect/configs/appsettings.json:/app/appsettings.json \
           --restart=always --network host dockertestconnect:latest

[http://192.168.10.16:8081/index.html](http://192.168.10.16:8081/index.html)