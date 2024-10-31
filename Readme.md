# TeamsEats
TeamsEats is an application designed to help order food at work, integrates with Teams app, and uses SSO.

## Projects
This repository contains the following projects:

- **TeamsApp**: Teams app manifest.
- **TeamsEats.Application**: Application logic and use cases.
- **TeamsEats.Domain**: Models, enums, and interfaces.
- **TeamsEats.Infrastructure**: Implementation of services and database access.
- **TeamsEats.Server**: API for the application.
- **TeamsEats.Client**: Frontend part of the application.

## API Endpoints

### Base URL
The base URL for all API endpoints is: `https://localhost:7125`

### Endpoints

#### 1. Get Order Summaries
- **URL**: `/order`
- **Method**: `GET`
- **Description**: Get a summary of orders.
- **Response**:
    ```json
    [
            {
                    "id": 0,
                    "isOwner": true,
                    "isParticipating": true,
                    "authorName": "string",
                    "authorPhoto": "string",
                    "deliveryCost": 0,
                    "restaurant": "string",
                    "status": 0,
                    "closingTime": "2024-10-28T09:41:11.857Z"
            }
    ]
    ```

#### 2. Get Order Summary
- **URL**: `/order/{id}`
- **Method**: `GET`
- **Description**: Get a summary of a specific order.
- **Response**:
    ```json
    {
            "id": 0,
            "isOwner": true,
            "isParticipating": true,
            "authorName": "string",
            "authorPhoto": "string",
            "deliveryCost": 0,
            "restaurant": "string",
            "status": 0,
            "closingTime": "2024-10-28T09:43:16.000Z"
    }
    ```

#### 3. Get Detailed Order
- **URL**: `/order/{id}/detail`
- **Method**: `GET`
- **Description**: Get detailed information about an order, including its items.
- **Response**:
    ```json
    {
            "id": 0,
            "isOwner": true,
            "authorName": "string",
            "authorPhoto": "string",
            "restaurant": "string",
            "phoneNumber": "string",
            "bankAccount": "string",
            "minimalPrice": 0,
            "deliveryCost": 0,
            "minimalPriceForFreeDelivery": 0,
            "items": [
                    {
                            "id": 0,
                            "authorPhoto": "string",
                            "authorName": "string",
                            "dish": "string",
                            "price": 0,
                            "isOwner": true,
                            "orderId": 0,
                            "additionalInfo": "string"
                    }
            ],
            "status": 0,
            "closingTime": "2024-10-28T09:45:21.085Z"
    }
    ```

#### 4. Create Order
- **URL**: `/order`
- **Method**: `POST`
- **Description**: Create a new order.
- **Request Body**:
    ```json
    {
            "phoneNumber": "string",
            "bankAccount": "string",
            "restaurant": "string",
            "minimalPrice": 0,
            "deliveryFee": 0,
            "minimalPriceForFreeDelivery": 0,
            "closingTime": "2024-10-28T09:42:51.122Z"
    }
    ```
- **Response**: `200 Success`

#### 5. Update Order
- **URL**: `/order/{id}`
- **Method**: `PATCH`
- **Description**: Update an existing order.
- **Request Body**:
    ```json
    {
            "id": 0,
            "status": 0
    }
    ```
- **Response**: `200 Success`

#### 6. Delete Order
- **URL**: `/order/{id}`
- **Method**: `DELETE`
- **Description**: Delete an order.
- **Response**: `200 Success`

#### 7. Create Item
- **URL**: `/item`
- **Method**: `POST`
- **Description**: Create a new item.
- **Request Body**:
    ```json
    {
            "orderId": 0,
            "dish": "string",
            "price": 0,
            "additionalInfo": "string"
    }
    ```
- **Response**: `200 Success`

#### 8. Update Item
- **URL**: `/item/{id}`
- **Method**: `PATCH`
- **Description**: Update an existing item.
- **Request Body**:
    ```json
    {
            "id": 0,
            "orderId": 0,
            "dish": "string",
            "price": 0,
            "additionalInfo": "string"
    }
    ```
- **Response**: `200 Success`

#### 9. Delete Item
- **URL**: `/item/{id}`
- **Method**: `DELETE`
- **Description**: Delete an item.
- **Response**: `200 Success`

#### 10. Comment Item
- **URL**: `/item/{id}/comments`
- **Method**: `POST`
- **Description**: Send a comment to the person who owns this item.
- **Request Body**:
    ```json
    {
            "itemId": 0,
            "message": "string"
    }
    ```
- **Response**: `200 Success`
