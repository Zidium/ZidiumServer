#!/bin/sh

if [ $# -eq 0 ]; then

_term() { 
  echo "Stopping all..." 
  kill -TERM "$child_dispatcher"
  kill -TERM "$child_user_account"
  kill -TERM "$child_agent"
}

trap _term SIGTERM

echo "Starting dispatcher...";
cd Dispatcher
dotnet Zidium.Dispatcher.dll &
child_dispatcher=$!
cd ..
echo "Dispatcher started, $child_dispatcher";

echo "Starting user account...";
cd UserAccount
dotnet Zidium.UserAccount.dll &
child_user_account=$!
cd ..
echo "User account started, $child_user_account";

echo "Starting agent...";
cd Agent
dotnet Zidium.Agent.dll --service &
child_agent=$!
cd ..
echo "Agent started, $child_agent";

echo "Ready to work!";
 
wait "$child_dispatcher"
echo "Dispatcher stopped"

wait "$child_user_account"
echo "User account stopped"

wait "$child_agent"
echo "Agent stopped"

else
"$@"
fi