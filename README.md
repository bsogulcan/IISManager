# IISManager
This project provides IIS management with API calls and aims to save time for programmers. It must run as administrator.


## Installation 

For now, you can install only by downloading the release file. I am working on npm package.
After downloading the release file, you should configure ip address and port information on the appsettings.json file.
After completing all the steps, just run the application as admin.

```bash 
  "App": {
    "IpAddress": "http://192.168.1.26",
    "Port": 3300
  },
```
## Usage
You can use the CLI application for easy and fast use
[IISManagerCli](https://github.com/bsogulcan/IISManagerCli).
Otherwise you can also use it with api requests.
  
## API Usage

#### Get All Sites

```http
  GET /IIS/GetAll
```

#### Get Site

```http
  GET /IIS/Get/{id}
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `id` | `int` | Id  |


#### Start Site

```http
  POST /IIS/Start
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `id` | `int` | Id  |

#### Stop Site

```http
  POST /IIS/Start
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `id` | `int` | Id  |

#### Create Site

```http
  POST /IIS/CreateFormData
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `name` | `string` | [FromForm] Name  |
| `port` | `int` | [FromForm] Port  |
| `bindingInformation` | `string` | [FromForm] Ip Address  |
| `file` | `file` | [FromForm] Zip file of publish folder  |


#### Update(Deploy) Site

```http
  POST /IIS/Deploy
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `id` | `int` | [FromForm] Id  |
| `file` | `file` | [FromForm] Zip file of publish folder  |

