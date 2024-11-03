docker ps
docker network inspect docker-network
docker network create docker-network
#docker network connect docker-network postgres1
docker pull postgres
docker run --name postgres1 -e POSTGRES_PASSWORD=mysecretpassword --network docker-network -d postgres
docker build -t nochainswap-api -f NoChainSwap.API\Dockerfile .
docker run --name nochainswap-api1 -p 8080:8080 -e ASPNETCORE_URLS="https://+" -e ASPNETCORE_HTTPS_PORTS=8080 --network docker-network nochainswap-api &

docker build -t nochainswap-app .
docker run --name nochainswap-app1 -p 80:3001 nochainswap-app