## Running the API Projects

In order to run the Game API and the Bot API locally, you need to have **[dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/)** installed on your machine

After installing the dapr CLI, you need to **[initialize dapr](https://docs.dapr.io/getting-started/install-dapr-selfhost/)**. In order to initialize it, make sure that **[Docker](https://www.docker.com/products/docker-desktop/)** is installed and the Docker Engine is running.

After you initialized dapr, you can run the API projects by opening a terminal for each of the projects that run dapr.

Open a terminal in 

`src/[Module]/[Exercise]/WebAPI`

and enter the following command:

`dapr run --app-id game-api --app-port 5015 --dapr-http-port 3500 --scheduler-host-address "" -- dotnet run`

Then open another terminal in 

`src/[Module]/[Exercise]/BotAPI`

and enter the following command:


`dapr run --app-id bot-api --app-port 5080 --dapr-http-port 3501 --scheduler-host-address "" -- dotnet run`

After running the commands, the two projects should run in the background. To stop them from running, you can press `CTRL+C`.

With the APIs running, you can now run the Web app.