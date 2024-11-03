# Decentralized Cross-Chain Exchange

A decentralized cross-chain exchange built using **.NET Core** that allows users to trade cryptocurrencies across different blockchains without relying on a centralized authority. The exchange operates peer-to-peer, ensuring security and privacy for users while maintaining full control over their funds.

## Features

- **Cross-Chain Trading**: Trade assets across different blockchains.
- **Decentralized**: No central authority or third party needed for trades.
- **Secure**: All transactions are cryptographically signed and validated.
- **P2P Network**: Built using a peer-to-peer architecture to ensure privacy and censorship resistance.
- **Open-Source**: Fully transparent and verifiable by the community.

## Technology Stack

- **.NET Core**: Backend logic and API services.
- **Stacks.js**: Blockchain interactions and smart contract management.
- **Docker**: Containerization for easy deployment.

## Prerequisites

Before running the project, ensure you have the following installed:

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Getting Started

### Installation

1. Create Docker Network

    ```bash
	docker network create docker-network
    ```

2. Install Postgres Database

    ```bash
	docker pull postgres
	docker run --name postgres1 -e POSTGRES_PASSWORD=mysecretpassword --network docker-network -d postgres
    ```

3. Install .NET Core Restful API

    ```bash
	docker build -t nochainswap-api -f NoChainSwap.API/Dockerfile .
	docker run --name nochainswap-api1 -p 8080:443 -e ASPNETCORE_URLS="https://+" -e ASPNETCORE_HTTPS_PORTS=8080 --network docker-network nochainswap-api &
    ```

4. Install STX Wallet microservice:

    ```bash
	docker build -t stacks-wallet .
	docker run --name wallet-stx -p 3000:3000 --network docker-network stacks-wallet
    ```
	
5. Install React Frontend:

    ```bash
	docker build -t nochainswap-app .
	docker run --name nochainswap-app1 -p 443:443 nochainswap-app
    ```

3. Once the application is up and running, you can access it in your browser at `http://localhost:5000`.

### Configuration

By default, the application uses a test environment for the supported blockchains. To configure the exchange for production or add more chains, modify the environment variables in the `docker-compose.yml` file.

For example:

```yaml
environment:
  - NETWORK=mainnet
  - SUPPORTED_CHAINS=bitcoin,ethereum,stacks
  - API_KEY=your-api-key
