import { UUID } from "crypto";
import IPark from "./park";

export interface CartItem {
    park: IPark;
    numDays: number;
    numAdults: number;
    numKids: number;
}

export interface Booking {
    adults: number;
    children: number;
    cartId: UUID;
    parkId: UUID;
    numDays: number;
    startDate: Date;
}