docker ps
docker network inspect docker-network
docker network create docker-network
docker network connect docker-network postgres1
docker build -t nochainswap-api -f NoChainSwap.API\Dockerfile .
docker run --name nochainswap-api1 -p 8080:8080 --network docker-network nochainswap-api &

docker build -t nochainswap-app .
docker run --name nochainswap-app1 -p 80:3001 nochainswap-app