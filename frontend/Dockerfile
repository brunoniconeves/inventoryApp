FROM node:20-slim as build

WORKDIR /build

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

FROM nginx:stable-alpine

COPY --from=build /build/build /usr/share/nginx/html

EXPOSE 3000

CMD ["nginx", "-g", "daemon off;"] 