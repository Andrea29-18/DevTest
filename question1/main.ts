export function isValidWalk(walk: string[]) {

    //First, make sure there are 10 steps
    if (walk.length !== 10) {
        return false;
    }

    //Count the number of steps in each direction and compare them
    const northSteps = walk.filter(step => step === 'n').length;
    const southSteps = walk.filter(step => step === 's').length;
    const eastSteps = walk.filter(step => step === 'e').length;
    const westSteps = walk.filter(step => step === 'w').length;

    /** 
        To return to the starting point, the number of north steps must equal the number of south steps
        And the number of east steps must equal the number of west steps
    **/
    return northSteps === southSteps && eastSteps === westSteps;

}
