#!/bin/bash
 echo "=====================Removing containers and images========================"
 docker rm -f $(docker ps -aq)
 docker rmi $(docker images -q)