#!/bin/bash
 echo "=====================Starting ELK stack========================"``
 docker-compose -f ./docker-compose-elk.rpi.yaml up -d
 echo "=====================Starting========================"
 docker-compose -f ./docker-compose.dependencies.yaml up -d
 docker-compose -f ./docker-compose.yaml up --build