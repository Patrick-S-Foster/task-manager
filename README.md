# How to run the Task Manager application

1. Ensure that Docker is installed locally (tested with version 27.5.1)
1. Ensure that git is installed locally (tested with version 2.46.2)
1. Ensure that .NET 9.0 is installed locally (tested with version 9.0.203)
1. In an empty directory, run the command `git clone https://github.com/Patrick-S-Foster/task-manager.git`
1. Inside the cloned repository, ensure that the `main` branch is checked-out by running the command `git switch main`
1. Run the command `docker compose up -d --build`
1. Wait for all containers to be downloaded and initialized (this may take some time)
1. Navigate to `~/TaskManager/Taskmanager.Client/` and run `dotnet run`