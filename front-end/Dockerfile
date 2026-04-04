# Production build served by nginx
FROM node:20-alpine AS builder

ARG VITE_API_URL=""
ENV VITE_API_URL=$VITE_API_URL

WORKDIR /app

# Install dependencies (use ci for reproducible installs)
COPY package.json package-lock.json* ./
# Install build dependencies required for native modules on Alpine
RUN apk add --no-cache python3 build-base g++ make git libc6-compat \
  && npm ci --silent

# Copy source and build
COPY . .
RUN npm run build

# Use nginx to serve the built files
FROM nginx:alpine
COPY --from=builder /app/dist /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
