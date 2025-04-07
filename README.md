# Orders API

## Overview
This project is a demo API that handles orders using multiple microservices. Each microservice is responsible for a specific domain: addresses, users, products, and orders. 
An API Gateway orchestrates the communication between these services via HTTP calls over REST endpoints. At this point, no autentication mechanism has been put in place.

## Architecture
- **Microservices**:
  - **Address Service**: Manages address-related operations.
  - **User Service**: Manages user-related operations.
  - **Product Service**: Manages product-related operations.
  - **Order Service**: Manages order-related operations.
  - **Order API Gate**: Orchestrates requests to the appropriate microservice.

## Database
Each microservice uses SQLite for storage, with automatic migrations run at each startup when in Debug mode.

## Development Setup

### Prerequisites
- Docker
- [Docker Compose](https://docs.docker.com/compose/)

### From Visual Studio
1. Clone the repository.
2. Navigate to the project directory.
3. Open the solution in Visual Studio.
4. Run the application in Development mode.
5. You will redirected to the [swagger](https://swagger.io/) page of the Api Gateway.
6. Use the provided REST endpoints to interact with the microservices.

### From docker-compose
1. Clone the repository.
2. Navigate to the project directory.
3. Open a terminal pointing to that folder.
4. Run the command `docker-compose up`
5. When the deployment is completed, open a Browser
6. Point to the url `http://localhost:<dinamically-assigned-port>/swagger/index.html`

**TODO**: check the name assigned to the gateway container!!!! 

**Note**: to get the port where the container is exposed, you should [inspect](https://docs.docker.com/reference/cli/docker/inspect/) the `orderapigate` container. Another option is setting a static bind by editing the `docker-compose.override.yml` file under the section `orderapigate:ports`, and replace the value "8080" with "<port-of-your-choice>:8080". Please ensure that the port is available on your machine.

## Design Decisions for the Order API Service
### Communication Between Services
The decision to use only API calls for service communication was made to simplify the overall architecture. This approach ensures that each service remains decoupled and can be developed, deployed, and scaled independently. To improve response times and reduce traffic, a layer of caching could be introduced at a later stage as a first improvement step. Caching frequently accessed data can significantly enhance performance and reduce the load on the underlying services.

## Stateless API Gateway
The API gateway has been designed to be stateless to allow it to scale easily. By not storing any session data, the gateway can handle a large number of requests and distribute the load across multiple instances.

## Order Microservice
### Data Validation
The order microservice interacts with the API gateway to gather data when validating requests. This ensures that the user, address, and products provided in the request exist and are valid.

### Data Storage
When writing data, the order microservice stores a minimal set of information. However, it retains some product data (name, price, and quantity) to ensure that the price remains fixed and that this information is available even after the eventual deletion of the original data.
The order service is intended to serve as a store for the history of orders. It relies on external references (IDs) to users and addresses to reference them. This design allows the service to maintain a record of transactions without storing redundant data.

### Soft Deletion
Soft deletion was chosen to allow for future analysis of canceled orders. This approach marks records as deleted without actually removing them from the database, enabling historical analysis and auditing.

### Data Aggregation
The order service does not return user or address details when queried. This composition is reserved for another component (out of the scope of this project) or an external service that can aggregate data from multiple sources.

## Endpoints
List of the endpoint for the OrderApiGate project:

### User Service
- **GET `/users`**: Read all users.
    - **Response**:  List of users or `ProblemDetail`.
- **GET `/users/:id`**: Read one user.
  - **Response**: User details or `ProblemDetail`.
  - **Response**: `Result` object with `message`, `value` (ID of the created user), and `success`. The ID is also returned in a header.
- **POST `/users`**: Create a new user.
  - **Response**: `Result` object with `message`, `value` (ID of the created user), and `success`. The ID is also returned in a header.
- **PUT `/users/:id`**: Update an existing user.
  - **Request Body**: JSON representation of the user.
  - **Response**: No content or `ProblemDetail` in case of failure.
- **DELETE `/users/:id`**: Delete a user.
  - **Response**: No content or `ProblemDetail` in case of failure.

### Address Service
- **GET `/addresses`**: Read all addresses.
  - **Response**: List of addresses or `ProblemDetail`.
- **GET `/addresses/:id`**: Read one address.
  - **Response**: Address details or `ProblemDetail`.
- **POST `/addresses`**: Create a new address.
  - **Request Body**: JSON representation of the address.
  - **Response**: `Result` object with `message`, `value` (ID of the created address), and `success`. The ID is also returned in a header.
- **PUT `/addresses/:id`**: Update an existing address.
  - **Request Body**: JSON representation of the address.
  - **Response**: No content or `ProblemDetail` in case of failure.
- **DELETE `/addresses/:id`**: Delete an address.
  - **Response**: No content or `ProblemDetail` in case of failure.

### Product Service
- **GET `/products`**: Read all products.
  - **Response**: List of products or `ProblemDetail`.
- **GET `/products/:id`**: Read one product.
  - **Response**: Product details or `ProblemDetail`.
- **POST `/products`**: Create a new product.
  - **Request Body**: JSON representation of the product.
  - **Response**: `Result` object with `message`, `value` (ID of the created product), and `success`. The ID is also returned in a header.
- **PUT `/products/:id`**: Update an existing product.
  - **Request Body**: JSON representation of the product.
  - **Response**: No content or `ProblemDetail` in case of failure.
- **DELETE `/products/:id`**: Delete a product.
  - **Response**: No content or `ProblemDetail` in case of failure.

### Order Service
- **GET `/orders`**: Read all orders.
  - **Response**: List of orders or `ProblemDetail`.
- **GET `/orders/:id`**: Read one order.
  - **Response**: Order details or `ProblemDetail`.
- **POST `/orders`**: Create a new order.
  - **Request Body**: JSON representation of the order request. It will just include the ids of the related entities, and the quantity of items.
  - **Response**: `Result` object with `message`, `value` (ID of the created order), and `success`. The ID is also returned in a header.
- **PUT `/orders/:id`**: Update an existing order.
  - **Request Body**: JSON representation of the order request. It will just include the ids of the related entities, and the quantity of items.
  - **Response**: No content or `ProblemDetail` in case of failure.
- **DELETE `/orders/:id`**: Delete an order.
  - **Response**: No content or `ProblemDetail` in case of failure.

## Data 

### Address Model

- Endpoint: **GET** /addresses
- Endpoint: **GET** /addresses/:id
   - **Response example (single item):**
   ```json
    {
        "addressId": 1,
        "street": "New Street",
        "city": "Sample City",
        "state": "Sample State",
        "country": "Sample Country",
        "zipCode": "12345"
    }
    ```
   - **Response Body Schema:**

   | Field |	Type	| Description |
   |-------|------------|-------------|
   | id	| Integer	| Unique identifier for the address |
   | street	| String	| Street address |
   | city	| String	| City name |
   | state	| String	| State name |
   | country	| String	| Country name |
   | zipCode	| String	| Postal code |

- Endpoint: **POST** /addresses
- Endpoint: **PUT** /addresses/:id
    - **Request example (single item):**
    ```json
    {
        "street": "string",
        "city": "string",
        "state": "string",
        "country": "string",
        "zipCode": "12345"
    }
    ```
    
    - **Request Body Schema:**

    | Field |	Type	| Description |
    |-------|-----------|-------------|
    | street	| String	| Street address |
    | city	| String	| City name |
    | state	| String	| State name |
    | country	| String	| Country name |
    | zipCode	| String	| Postal code |



### User Model

- Endpoint: **GET** /users
- Endpoint: **GET** /users/:id
   - **Response example (single item):**
   ```json
   {
       "userId": 2,
       "name": "Jane Smith",
       "email": "jane.smith@example.com"
   }
   ```
   - **Response Body Schema:**

   | Field |	Type	| Description |
   |-------|------------|-------------|
   | userId	| Integer	| Unique identifier for the user |
   | name	| String	| Name of the user |
   | email	| String	| Email address of the user |


- Endpoint: **POST** /users
- Endpoint: **PUT** /users/:id
    - **Request example (single item):**
    ```json
    {
        "name": "Jane Smith",
        "email": "jane.smith@example.com"
    }
    ```
    
    - **Request Body Schema:**

    | Field |	Type	| Description |
    |-------|-----------|-------------|
    | name	| String	| Name of the user |
    | email	| String	| Email address of the user |

### Category Model

- Endpoint: **GET** /categories
- Endpoint: **GET** /categories/:id
   - **Response example (single item):**
   ```json
   {
       "categoryId": 1,
       "name": "Cat1"
   }
   ```
   - **Response Body Schema:**

   | Field |	Type	| Description |
   |-------|------------|-------------|
   | categoryId	| Integer	| Unique identifier for the category |
   | name	| String	| Name of the category |


- Endpoint: **POST** /categories
- Endpoint: **PUT** /categories/:id
    - **Request example (single item):**
    ```json
    {
        "name": "Cat1"
    }
    ```
    
    - **Request Body Schema:**

    | Field |	Type	| Description |
    |-------|-----------|-------------|
    | name	| String	| Name of the categories |


### Product Model
- Endpoint: **GET** /products
- Endpoint: **GET** /products/:id

   - **Response example (single item):**
   ```json
   {
       "productId": 2,
       "name": "Jane Smith",
       "price": 10.2,
       "category":
       {
          "categoryId": 1,
          "name": "cat1"
       },
       "categoryId": 1
   }
   ```
   - **Response Body Schema:**

   | Field |	Type	| Description |
   |-------|------------|-------------|
   | id	| Integer	| Unique identifier for the product |
   | name	| String	| Name of the product |
   | price	| Number	| Price of the product |
   | category	| Object	| Category details |
   | categoryId	| String	| Unique identifier for the category |

   - **Category Schema:**

   | Field |	Type	| Description |
   |-------|------------|-------------|
   | categoryId	| Integer	| Unique identifier for the product |
   | name	| String	| Name of the category |

- Endpoint: **POST** /products
- Endpoint: **PUT** /products/:id
    - **Request example (single item):**
    ```json
    {
        "name": "Product One",
        "price": 10.2,
        "categoryId": 1
    }
    ```
    
    - **Request Body Schema:**

    | Field |	Type	| Description |
    |-------|-----------|-------------|
    | name	| String	| Name of the product |
    | price	| Number	| Price of the product |
    | categoryId	| String	| Unique identifier for the category |


### Order Model
- Endpoint: **GET** /orders
- Endpoint: **GET** /orders/:id

   - **Response example (single item):**
   ```json
   {
       "orderId": 1,
       "userId": 2,
       "addressId": 3,
       "orderDetails":
       [
            {
                "productId": 1,
                "productName": "prod 1",
                "quantity": 6,
                "unitPrice": 5.3
            },
            {
                "productId": 3,
                "productName": "prod 3",
                "quantity": 1,
                "unitPrice": 12.0
            }
       ]
   }
   ```
   - **Response Body Schema:**

   | Field |	Type	| Description |
   |-------|------------|-------------|
   | userId	| Integer	| Unique identifier for the product |
   | addressId	| String	| Name of the product |
   | productIds	| Object	| Price of the product |
   | category	| Object	| Category details |
   | categoryId	| String	| Unique identifier for the category |

- Endpoint: **POST** /orders
- Endpoint: **PUT** /orders/:id
    - **Request example (single item):**
    ```json
    {
        "userId":1,
        "addressId": 2,
        "orderDetails": [
            {
                "productId": 1,
                "quantity": 6
            },
            {
                "productId": 3,
                "quantity": 1
            }
        ]
    }
    ```
    
   - **Request Body Schema:**

   | Field |	Type	| Description |
   |-------|------------|-------------|
   | userId	| Integer	| Unique identifier for the order |
   | addressId	| Integer	| Unique identifier for the address  |
   | orderDetails	| Object	| Price of the product |
   
   - **ProductIds Schema:**

   | Field |	Type	| Description |
   |-------|------------|-------------|
   | productId	| Integer	| Unique identifier for the product |
   | quantity	| Integer	| Quantity ordered for this product |


## License
This project is licensed under the MIT License.

## Contact
For any inquiries, please contact [your-email@example.com](mailto:your
