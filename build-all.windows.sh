#!/bin/bash
 echo "=====================Starting ELK stack========================"``
 docker-compose -f ./docker-compose-elk.windows.yaml up -d
 echo "=====================Starting========================"
 docker-compose -f ./docker-compose.dependencies.yaml run --rm start_dependencies
 docker-compose -f ./docker-compose.yaml -f ./docker-compose.windows.yaml up --build