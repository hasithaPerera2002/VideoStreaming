#!/bin/bash
set -e  # exit immediately if any command fails

echo "🌩  Starting Floci (local AWS emulator)..."
docker compose up -d floci

echo "⏳ Waiting for Floci to be ready..."
until curl -s http://localhost:4566/_localstack/health > /dev/null 2>&1 || curl -s http://localhost:4566 > /dev/null 2>&1; do
  sleep 1
  echo "   still waiting..."
done
echo "✅ Floci is up"

echo "🏗  Applying Terraform infrastructure..."
cd infra
terraform init -input=false
terraform apply -auto-approve
cd ..

echo "🚀 Starting server + client..."
docker compose up --build server client