# Usar a imagem Node.js
FROM node:18

# Definir o diretório de trabalho
WORKDIR /app

# Copiar arquivos package.json e package-lock.json
COPY package*.json ./

# Instalar dependências
RUN npm install

# Copiar o restante da aplicação
COPY . .

# Expor a porta (caso seja necessário)
EXPOSE 80

# Comando para rodar o script Node.js
CMD ["npm", "start"]
