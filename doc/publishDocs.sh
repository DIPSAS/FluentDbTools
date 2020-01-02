# If any commands fail, halt immediately:
set -eu

# Run DocFX through Mono in the project root (assumed to be the current directory)
mono /opt/docfx/docfx.exe

git config user.email "deploy@travis-ci.org"
git config user.name "Deployment Bot"

git config credential.helper '!auth() { sleep 1; echo "username=${DOCUMENTATION_DEPLOY_USERNAME}"; echo "password=${DOCUMENTATION_DEPLOY_TOKEN}"; }; auth'

# Force-reset the gh-pages branch to the latest master, and check it out:
git branch -f gh-pages master
git checkout gh-pages

# Commit the static documentation under docs/ and push it to the gh-pages branch:
git add docs/
git commit -m "Update documentation"

git push -f origin gh-pages