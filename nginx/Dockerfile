FROM nginx:1.27-alpine

COPY nginx/nginx.conf /etc/nginx/nginx.conf

RUN mkdir -p /var/cache/nginx/public_api && \
    chown -R nginx:nginx /var/cache/nginx

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
