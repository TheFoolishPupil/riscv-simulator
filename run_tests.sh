#!/bin/bash

bold=`tput bold`
red=`tput setaf 1`
green=`tput setaf 2`
reset=`tput sgr0`

complete=1

scriptDir=$(dirname -- "$(greadlink -f -- "$BASH_SOURCE")")
cd "$scriptDir"
echo -e "\n${bold}Building Simulator 🛠${reset}\n"
dotnet build
cd "$scriptDir/tests"

echo -e "\n${bold}Running Tests 🏃${reset}\n"
for dir in */
do
    echo -e "${bold}Task ${dir:4:1}${reset}"
    cd "$dir"
    for f in *.bin
    do
        # Run the simluator on the task file
        ERROR=$(dotnet run -p ../../ $f 2>&1 > /dev/null)
        if [[ -n "$ERROR" ]] ; then
            complete=0
            # Print error
            printf "\n%-12s\t ${red}⚠️ Program crashed with following exception:${reset}\n" ${f%????}
            printf "____________________________________________\n"
            printf "$ERROR"
            printf "\n____________________________________________\n\n"

        else

            # Determine if the two result files are different
            DIFF=$(diff ${f%????}".res" ${f%????}"_.res")
            if [ "$DIFF" ]; then
                complete=0
                printf "%-12s\t ${red}Test Failed \xE2\x9D\x8C${reset}\n" ${f%????}
            else
                printf "%-12s\t ${green}Test Passed \xE2\x9C\x94${reset}\n" ${f%????}
            fi
        fi
    done
    echo ""
    cd ..
done
if [[ $complete == 1 ]] ; then
    echo "All tests passed! 🍾 🥂"
else
    echo "Not all tests passed... 😈"
fi
