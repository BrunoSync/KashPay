# ⚡ KashPay
 
> Digital wallet API with transfers, deposits, withdrawals and transaction history.  
> *Inspired by real-world systems like Nubank, PayPal and C6 Bank*
 
![.NET](https://img.shields.io/badge/.NET_10-512bd4?style=flat-square&logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169e1?style=flat-square&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ed?style=flat-square&logo=docker&logoColor=white)
![EF Core](https://img.shields.io/badge/EF_Core-512bd4?style=flat-square&logo=dotnet&logoColor=white)
 
---
 
## 📌 Overview
 
KashPay is a backend API that simulates a digital wallet system. Users can register, authenticate, deposit and withdraw funds, transfer money to other accounts and view their transaction history with cursor-based pagination.
 
---
 
## ✅ Features
 
- → JWT authentication with rotating refresh tokens
- → CPF and email login support with HMAC-SHA256 CPF hashing
- → Deposit, withdrawal and P2P transfers with business rules
- → Cursor-based pagination for transaction history
- → Pessimistic locking with SELECT FOR UPDATE preventing race conditions in concurrent transfers
- → Forgot password flow with 6-digit code sent via email
- → Global exception handler with standardized error responses
- → 88 unit tests covering entities, handlers and validators
---
 
## 🏛️ Architecture
 
```
src/
├── KashPay.API/            # Controllers, middleware, DI setup
├── KashPay.Application/    # Handlers, use cases, DTOs, validators
├── KashPay.Domain/         # Entities, value objects, domain rules
└── KashPay.Infrastructure/ # Repositories, EF Core, external services
```
 
---
 
## 🚀 Running locally
 
```bash
# clone the repository
git clone https://github.com/BrunoSync/KashPay
 
# create .env file based on the example
cp .env.example .env
 
# start with Docker
docker compose up --build
 
# API available at
http://localhost:5000/scalar
 
# Email UI (Mailpit) available at
http://localhost:8025
```
 
---
 
## 📡 Endpoints
 
### Auth
| Method | Route | Description |
|--------|-------|-------------|
| `POST` | /auth/register | Register a new user |
| `POST` | /auth/login | Login with email or CPF |
| `POST` | /auth/refresh | Rotate refresh token |
| `POST` | /auth/logout | Revoke all active tokens |
| `POST` | /auth/forgotpassword | Request password reset code via email |
| `POST` | /auth/resetpassword | Reset password using the 6-digit code |
 
### Wallet
| Method | Route | Description |
|--------|-------|-------------|
| `GET` | /wallet | Get current balance |
| `POST` | /wallet/deposit | Deposit funds |
| `POST` | /wallet/withdraw | Withdraw funds |
| `POST` | /wallet/transfer | Transfer to another account |
| `GET` | /wallet/transactions | Transaction history with cursor pagination |