# Instructions for Updating the Project After Pushing to GitHub

1. **Update the Prerequisites and Introduction Document:**

- In **Prerequisites and Introduction.md**, update Step 4.
- Replace the placeholder URL for the template repository with the actual GitHub path where students will clone the project.

2. **Check Workflow Files:**

- The workflow files are currently set to run on the `master` branch by default.
- If your branch name differs (e.g., `main` or another name), update the branch reference in both workflow files to match the correct branch name.

3. **Avoid Creating New Branches:**

- After pushing the code to the chosen branch, **do not create additional branches.**
- Creating new branches will trigger the workflow files, causing the placeholders `{Owner}` and `{Repo}` to be automatically replaced with values from your GitHub repository.
