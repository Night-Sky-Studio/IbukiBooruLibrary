type SharpNamespace = URL;

export enum ExtensionKind {
    SFW = "sfw", 
    NSFW = "nsfw"
}

export enum ExtensionType {
    XML = "xml", 
    JSON = "json"
}

export type Extension = {
    name: string;
    kind: ExtensionKind;
    api_type: ExtensionType;
    base_url: string;
    tags_separator: string;
}

/**
 * Used to import CSharp namespace into the JS context
 * @param namespace The namespace to import
 */
export function importNamespace(namespace: string): SharpNamespace;

export class URL {
    constructor(base_url: string, path: string);
    constructor(url: string);
    
    AppendQueryParam(key: string, value: string): void;
    AppendString(query: string): void;
    
    ToString(): string;
    
}
