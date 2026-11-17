import { Api } from "./Api.ts";

const api = new Api({
    baseUrl: "http://localhost:5214",
    baseApiParams: {
        credentials: "include"
    }
})

export default function useRainCrud(){
    
}