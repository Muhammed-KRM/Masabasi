# ==============================================================================
# Aşama 1: Node.js 22 Alpine ile Angular SSR Derlemesi
# ==============================================================================
FROM node:22-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build

# ==============================================================================
# Aşama 2: Hafif Node.js Alpine SSR Çalışma Zamanı
# ==============================================================================
FROM node:22-alpine AS runtime
WORKDIR /app

ENV NODE_ENV=production
ENV PORT=4000

# Güvenlik için yetkisiz kullanıcı oluştur
RUN addgroup -S masabasi && adduser -S masabasi -G masabasi

COPY --from=build --chown=masabasi:masabasi /app/dist/Masabasi.Frontend ./dist/Masabasi.Frontend
COPY --from=build --chown=masabasi:masabasi /app/package*.json ./

USER masabasi
EXPOSE 4000

CMD ["node", "dist/Masabasi.Frontend/server/server.mjs"]
