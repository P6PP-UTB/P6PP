FROM node:20-alpine AS build
WORKDIR /app
# Kopírování souborů pro instalaci závislostí
COPY package*.json ./
RUN npm install
# Kopírování zbytku kódu
COPY . .
# Sestavení Angular aplikace
# RUN CI=false npm run build || echo "Build errors were ignored"
RUN npm run build
# Vypsat obsah dist adresáře pro kontrolu
RUN ls -la dist/

# Produkční image založený na Nginx
FROM nginx:alpine AS final
# Kopírování sestavené aplikace do nginx - upravená cesta
COPY --from=build /app/dist/client-app /usr/share/nginx/html/client-app
RUN chmod -R 755 /usr/share/nginx/html && \
    chown -R nginx:nginx /usr/share/nginx/html
# Zkontrolovat obsah webového adresáře
RUN ls -la /usr/share/nginx/html/
# Kopírování vlastní Nginx konfigurace
COPY nginx.conf /etc/nginx/conf.d/default.conf
# Exponování portu pro komunikaci
EXPOSE 8011
# Spuštění Nginx serveru
CMD ["nginx", "-g", "daemon off;"]