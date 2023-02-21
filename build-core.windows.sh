#!/bin/bash
 echo "=====================Starting========================"
 docker-compose -f ./docker-compose.dependencies.yaml run --rm start_dependencies
 docker-compose -f ./docker-compose.yaml -f ./docker-compose.windows.yaml up --build