import { useParams } from "react-router-dom";
import { useGetGalleryPhotosQuery } from "../../services/GalleryApi";
import PhotoGallery from "./components/PhotoGallery";
import { getQRCodeUrl } from "../../utils/qRCodeHelper";
import QRCodeCard from "./components/QRCodeCard";

const HomePage = () => {
    const { galleryId } = useParams();
    const { data } = useGetGalleryPhotosQuery(galleryId || "", { skip: !galleryId });
    const qRCodeUrl = getQRCodeUrl()

    const hasPhotos = data && data.length > 0
    return (
        <div className="min-h-screen bg-black">
            {
                hasPhotos && (<PhotoGallery photoItems={data} />)
            }
            {
                qRCodeUrl && <QRCodeCard qRCodeUrl={qRCodeUrl} hasPhotos={hasPhotos} />
            }
        </div>
    )
}

export default HomePage