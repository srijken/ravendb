import commandBase = require("commands/commandBase");
import database = require("models/database");
import collection = require("models/collection");

class getCachedCollectionsCount extends commandBase {

    /**
	* @param ownerDb The database the collections will belong to.
	*/
    constructor(private ownerDb: database, private lastQueryDate: string) {
        super();

        if (!this.ownerDb) {
            throw new Error("Must specify a database.");
        }
    }

    execute(): JQueryPromise<any> {
        return this.query("/studio-tasks/collection/counts", {fromDate : this.lastQueryDate}, this.ownerDb);
    }
}

export = getCachedCollectionsCount;