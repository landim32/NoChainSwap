FROM nginx:latest

COPY --from=build /app/build /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 3001/tcp

CMD ["/usr/sbin/nginx", "-g", "daemon off;"]
