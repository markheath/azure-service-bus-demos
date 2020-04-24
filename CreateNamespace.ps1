az account show --query name

$resourceGroup = "servicebustest"
$location = "westeurope"

az group create -n $resourceGroup -l $location

# if already created, find out the name with 
$namespaceName = az servicebus namespace list -g $resourceGroup --query [].name -o tsv

# generate a random unique name
$namespaceName = "marksb$(Get-Random 10000)"
az servicebus namespace create -g $resourceGroup `
    -n $namespaceName -l $location --sku Standard

# create a servicebus queue
$queueName = "queue1"
az servicebus queue create -g $resourceGroup `
    --namespace-name $namespaceName -n $queueName

# get hold of the connection string
$connectionString = az servicebus namespace authorization-rule keys list `
    -g $resourceGroup --namespace-name $namespaceName `
    -n RootManageSharedAccessKey `
    --query primaryConnectionString `
    --output tsv

dotnet user-secrets init
dotnet user-secrets set ServiceBusConnectionString $connectionString
