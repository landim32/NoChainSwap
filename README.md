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

1. Install Postgres Database

    ```bash
	docker pull postgres
	docker run --name postgres1 -e POSTGRES_PASSWORD=mysecretpassword --network docker-network -d postgres
    ```

2. Build and run the application using Docker:

    ```bash
    docker-compose up --build
    ```

    This command will download the required Docker images, build the application, and start the containers.

3. Once the application is up and running, you can access it in your browser at `http://localhost:5000`.

### Configuration

By default, the application uses a test environment for the supported blockchains. To configure the exchange for production or add more chains, modify the environment variables in the `docker-compose.yml` file.

For example:

```yaml
environment:
  - NETWORK=mainnet
  - SUPPORTED_CHAINS=bitcoin,ethereum,stacks
  - API_KEY=your-api-key
