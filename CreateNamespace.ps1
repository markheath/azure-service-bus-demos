az account show --query name

$resourceGroup = "servicebustest"
$location = "westeurope"

az group create -n $resourceGroup -l $location

# generate a random unique name
$namespaceName = "marksb$(Get-Random 10000)"
az servicebus namespace create -g $resourceGroup `
    -n $namespaceName -l $location --sku Standard

# create a servicebus queue
$queueName = "queue1"
az servicebus queue create -g $resourceGroup `
    --namespace-name $namespaceName -n $queueName

# get hold of the connection string
az servicebus namespace authorization-rule keys list `
    -g $resourceGroup --namespace-name $namespaceName `
    -n RootManageSharedAccessKey `
    --query primaryConnectionString `
    --output tsv