<!-- Centering the title -->
<h1 align="center">WebApiPortfolioApp</h1>

<!-- Introduction -->
<p align="justify">
Welcome to the WebApiPortfolioApp! This project demonstrates my skills in creating a RESTful API using modern technologies. The application is designed to fetch and showcase data from third-party APIs and provide various CRUD operations.
</p>

<!-- Key API Endpoints -->
<h2>Key API Endpoints</h2>

<p align="justify">
<b>Authentication</b><br>
POST /api/Identity/login: Authenticate a user and retrieve a custom JWT. Login with parameters:
<ul>
    <li>Email</li>
    <li>Password</li>
</ul>
POST /api/Identity/register: Register a new user with parameters:
<ul>
    <li>Name</li>
    <li>Email</li>
    <li>Password</li>
    <li>ConfirmPassword</li>
    <li>SubscribeToMailingList</li>
</ul>
</p>

<!-- Health Check -->
<p align="justify">
<b>Health Check</b><br>
GET /api/HealthCheck: Monitor the overall health and readiness of the application.
</p>

<!-- Products -->
<p align="justify">
<b>Products</b><br>
GET /api/Products/GetProducts: Retrieve products using parameters:
<ul>
    <li>SearchProduct (required): The more specific the product name, the more accurate the results.</li>
    <li>Store: Filter results by a specific store.</li>
    <li>NumberOfResults: Default is 100 if SearchProduct is general.</li>
</ul>
Results are saved to the database in the "Search History" table with columns: UserId, SearchString, SearchDate, Store, Price, Created.
</p>

<p align="justify">
GET /api/Products/GetAllProductsFromDb: Fetch all data from the "Search History" table.
</p>

<p align="justify">
GET /api/Products/FindAndAddProductsToNewsletter: Allows a logged-in user to subscribe to a newsletter for price changes of a given product. Users can be notified about price changes in a specific store. Data is saved to the "ProductSubscription" table with columns: ProductName, UserId, Store, Price, Created.
</p>

<p align="justify">
GET /api/Products/UpdateProductPrice: Since the third-party API doesn't support bulk operations, this endpoint retrieves UserId and loops through the "ProductSubscription" table, making API requests with ProductName corresponding to UserId. The price of a given product is compared with the one in "ProductSubscription", and updates are made if needed.
</p>

<!-- Background Service -->
<p align="justify">
<b>Background Service</b><br>
There is also a background service that runs every Friday at 12:00. It checks the price of “Hansa Mango Ipa 0,5l boks”. Data is saved to the "ProductSubscription" table. If the price drops, users receive an email notification.
</p>

<!-- Technologies Used -->
<h2>Technologies Used</h2>
<p align="justify">
<ul>
    <li>ASP.NET Core Web API</li>
    <li>Entity Framework</li>
    <li>ASP.NET Identity Core</li>
    <li>SQL Server</li>
    <li>.NET Quartz</li>
    <li>MediatR</li>
    <li>AutoMapper</li>
    <li>MailKit</li>
    <li>RestSharp</li>
</ul>
</p>

<!-- Installation -->
<h2>Installation</h2>
<p align="justify">
<ol>
    <li><b>Clone the repository</b>:
        <pre>
        git clone https://github.com/netycbo/WebApiPortfolioApp.git
        </pre>
    </li>
    <li><b>Navigate to the project directory</b>:
        <pre>
        cd WebApiPortfolioApp
        </pre>
    </li>
    <li><b>Restore dependencies</b>:
        <pre>
        dotnet restore
        </pre>
    </li>
    <li><b>Update the database</b>:
        <pre>
        dotnet ef database update
        </pre>
    </li>
    <li><b>Run the application</b>:
        <pre>
        dotnet run
        </pre>
    </li>
</ol>
</p>


<!-- Contact -->
<h2>Contact</h2>
<p align="justify">
For any questions or inquiries, please contact me at <a href="mailto:your-mikolaj.silinski@gmail">mikolaj.silinski@gmail.com</a>.
</p>
