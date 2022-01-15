#!/usr/bin/env bash
# Changed end-of-line endings to Unuix(LF) (Hint - Notepad++ => Edit-menu => EOL Conversion => Unuix(LF))
# If any commands fail, halt immediately: 
set -eu

# Run DocFX through Mono in the project root (assumed to be the current directory)
mono /opt/docfx/docfx.exe

if [ -z "$DOCUMENTATION_DEPLOY_USERNAME" ]; then 
    echo "DOCUMENTATION_DEPLOY_USERNAME not set - won't attempt to check in documentation"
    exit
fi

git config user.email "deploy@travis-ci.org"
git config user.name "Deployment Bot"

git config credential.helper '!auth() { sleep 1; echo "username=${DOCUMENTATION_DEPLOY_USERNAME}"; echo "password=${DOCUMENTATION_DEPLOY_TOKEN}"; }; auth'
#git config credential.helper '!auth() { sleep 1; echo "username=dips-aho"; echo "password=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"; }; auth'

# Force-reset the gh-pages branch to the latest master, and check it out:
git branch -f gh-pages master
git checkout gh-pages

# Commit the static documentation under docs/ and push it to the gh-pages branch:
git add docs/
git commit -m "Update documentation"

git push https://${DOCUMENTATION_DEPLOY_USERNAME}:${DOCUMENTATION_DEPLOY_TOKEN}@github.com/dipsas/FluentDbTools.git -f gh-pages
# git push -f origin gh-pages