import { Status } from "./Status";

export interface OrderData {
    id: number;
    isOwner: boolean;
    authorName: string;
    authorPhoto: string;
    currentDeliveryFee: number;
    currentPrice: number;
    minimalPrice: number;
    restaurant: string;
    status: Status;
    closingTime: Date;
    myCost: number;
}