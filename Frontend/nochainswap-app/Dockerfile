# Stage 1: Build the React Application
#FROM node:18
FROM nginx:alpine 

WORKDIR /app

#COPY package*.json ./

#RUN npm install

COPY . .
#COPY ./build /usr/share/nginx/html

#RUN npm run build

# Stage 2: Setup the Nginx Server to serve the React Application
#FROM nginx:alpine 

COPY build /usr/share/nginx/html

COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 3001

CMD ["nginx", "-g", "daemon off;"]