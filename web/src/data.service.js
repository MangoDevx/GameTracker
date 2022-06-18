import Database from 'better-sqlite3';

export class DataService {
    getDbData() {
        const db = new Database('../', { readonly: true, fileMustExist: true })
    }
}