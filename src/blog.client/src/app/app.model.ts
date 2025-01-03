export interface Author {
    name: string;
    bio: string;
    linkedin: string;
    github: string;
}

export interface BlogInfo {
    title: string;
    description: string;
}

export interface BlogData {
    author: Author;
    blog: BlogInfo;
}
