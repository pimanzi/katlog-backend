# Katlog API

A product catalogue and asset management platform built with ASP.NET Core, PostgreSQL, and Apache Kafka.

## Prerequisites

- .NET 10 SDK
- Docker Desktop
- PostgreSQL running locally on port 5432
- A Cloudinary account (for asset uploads)

# Project Structure

```text
katlog-backend/
├── Katlog.Api/         # ASP.NET Core Web API
├── Katlog.Consumer/    # Background worker service
├── Katlog.Shared/      # Shared event classes
└── docker-compose.yml  # Kafka and Kafka UI
```

## Local Setup

### 1. Clone the Repository

```bash
git clone https://github.com/pimanzi/katlog-backend.git
cd katlog-backend
```

### 2. Configure Katlog.Api

Create `Katlog.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=katlog;Username=YOUR_USERNAME;Password=YOUR_PASSWORD"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-minimum-32-characters",
    "Issuer": "katlog-api",
    "Audience": "katlog-client",
    "ExpiryInMinutes": 60
  },
  "CloudinarySettings": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  },
  "AdminSettings": {
    "Email": "admin@katlog.com",
    "Password": "Admin@1234",
    "FirstName": "Admin",
    "LastName": "User"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9093"
  },
  "EnableSwagger": true
}
```

### 3. Configure Katlog.Consumer

Create `Katlog.Consumer/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=katlog;Username=YOUR_USERNAME;Password=YOUR_PASSWORD"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9093",
    "GroupId": "katlog-asset-consumer-group"
  }
}
```

## Starting Kafka

From the repo root:

```bash
docker-compose up -d
```

This starts:
- Kafka broker on port 9093 (for local apps)
- Kafka UI at http://localhost:8080

To stop Kafka:

```bash
docker-compose down
```

## Starting the API

```bash
cd Katlog.Api
dotnet run
```

On startup the API will:
- Run database migrations automatically
- Seed Admin and User roles
- Seed a default admin user using credentials from AdminSettings

API runs at http://localhost:5235

API documentation available at [[http://localhost:5235/swagger/index.html](http://localhost:5235/swagger/index.html)]

## Starting the Consumer

Open a new terminal:

```bash
cd Katlog.Consumer
dotnet run
```

The consumer connects to Kafka and listens to the `catalogue.asset-events` topic. It writes a record to the `NotificationLogs` table when an asset is approved or rejected.

## Verifying Messages Are Flowing

# API Testing Guide

Follow the steps below to test the Katlog API.

## Step 1: Login

**Request**

```http
POST /api/auth/login
```

**Body**

```json
{
  "email": "YOUR_ADMIN_EMAIL",
  "password": "YOUR_ADMIN_PASSWORD"
}

Use the credentials you
configured in AdminSettings
of your appsettings.Development.json
```

**Next Step**

Copy the JWT token from the response and include it in the `Authorization` header for all subsequent requests.

```http
Authorization: Bearer {your-jwt-token}
```

---

## Step 2: Create a Brand

**Request**

```http
POST /api/brands
```

**Body**

```json
{
  "name": "Nike"
}
```

---

## Step 3: Create a Category

**Request**

```http
POST /api/categories
```

**Body**

```json
{
  "name": "Shoes"
}
```

---

## Step 4: Create a Product

**Request**

```http
POST /api/products
```

**Body**

```json
## Request Examples

### Create Product
```json
{
  "name": "Air Max 90",
  "productCode": "NK-001",
  "description": "Classic running shoe",
  "season": "Summer",
  "targetMarket": ["Men", "Unisex"],
  "brandId": 1,
  "categoryId": 1
}
```

**season** accepted values: `Spring` `Summer` `Autumn` `Winter`

**targetMarket** accepted values (send as array): `Men` `Women` `Boys` `Girls` `Unisex` `Adults` `All`



### Upload Asset
```

---

## Step 5: Upload an Asset

**Request**

```http
POST /api/assets


**Content-Type: multipart/form-data**

| Field | Required | Description |
|---|---|---|
| file | Yes | Image file to upload |
| productId | Yes | ID of the product |
| assetType | Yes | Type of asset (see below) |
| variantId | No | ID of the variant if asset belongs to one |
| title | No | Asset title |
| description | No | Asset description |
| tags | No | Comma separated list of tags |

**assetType** accepted values: `MainImage` `VariantImage` `LifestyleImage` `MarketingBanner` `SizeGuide` `TechnicalDocument`

```
## Step 6: Approve the Asset

**Request**

```http
POST /api/assets/{id}/approve
```

Replace `{id}` with the ID of the asset you want to approve.

---

## Request Flow

```text
Login
   ↓
Create Brand
   ↓
Create Category
   ↓
Create Product
   ↓
Upload Asset
   ↓
Approve Asset
```

### Step 7 - Verify in Kafka UI

1. Open http://localhost:8080
2. Select the katlog-local cluster
3. Navigate to Topics
4. Open catalogue.asset-events
5. You should see the AssetApproved message

### Step 8 - Verify NotificationLog

Check the consumer terminal for:
NotificationLog written: AssetApproved for Asset {id}, Product {id}

Or query directly in PostgreSQL:

```sql
SELECT * FROM "NotificationLogs";
```
